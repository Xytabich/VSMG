using ModelGenerator;
using System;
using System.Collections.Generic;
using Vintagestory.API.Client;

namespace VoxelCombinerGenerator
{
    internal class VoxelModelUtils
    {
        private const int IGONORE_X_MASK = ~(1 | (1 << 1));
        private const int IGONORE_Y_MASK = ~((1 << 2) | (1 << 3));
        private const int IGONORE_Z_MASK = ~((1 << 4) | (1 << 5));

        public static MaterialCuboidInfo[,,] VoxelsToCuboids(int[,,] voxels, IReadOnlyList<MaterialProperties> materials, bool cullBetweenTransparents, int sizeX, int sizeY, int sizeZ)
        {
            var cuboids = new MaterialCuboidInfo[sizeX, sizeY, sizeZ];
            sizeX--;
            sizeY--;
            sizeZ--;
            for(int x = 0; x <= sizeX; x++)
            {
                for(int y = 0; y <= sizeY; y++)
                {
                    for(int z = 0; z <= sizeZ; z++)
                    {
                        int material = voxels[x, y, z];
                        if(material >= 0)
                        {
                            int neighbors = 0;
                            if(x < sizeX && CullSide(material, voxels[x + 1, y, z], materials, cullBetweenTransparents)) neighbors |= 1;
                            if(x > 0 && CullSide(material, voxels[x - 1, y, z], materials, cullBetweenTransparents)) neighbors |= 1 << 1;
                            if(y < sizeY && CullSide(material, voxels[x, y + 1, z], materials, cullBetweenTransparents)) neighbors |= 1 << 2;
                            if(y > 0 && CullSide(material, voxels[x, y - 1, z], materials, cullBetweenTransparents)) neighbors |= 1 << 3;
                            if(z < sizeZ && CullSide(material, voxels[x, y, z + 1], materials, cullBetweenTransparents)) neighbors |= 1 << 4;
                            if(z > 0 && CullSide(material, voxels[x, y, z - 1], materials, cullBetweenTransparents)) neighbors |= 1 << 5;

                            var cuboid = new MaterialCuboidInfo(x, y, z, 1, 1, 1);
                            cuboid.neighbors = neighbors;
                            cuboid.material = material;
                            cuboids[x, y, z] = cuboid;
                        }
                    }
                }
            }
            return cuboids;
        }

        public static void MergeNeighbors(MaterialCuboidInfo[,,] cuboids, int sizeX, int sizeY, int sizeZ)
        {
            MergeNeighborsInternal(cuboids, 0, 0, 0, sizeX - 1, sizeY - 1, sizeZ - 1);
        }

        public static void MergeNeighbors(MaterialCuboidInfo[,,] cuboids, int split, int sizeX, int sizeY, int sizeZ)
        {
            MergeNeighbors(cuboids, split, split, split, sizeX, sizeY, sizeZ);
        }

        public static void MergeNeighbors(MaterialCuboidInfo[,,] cuboids, int splitX, int splitY, int splitZ, int sizeX, int sizeY, int sizeZ)
        {
            int xCount = (sizeX - 1) / splitX + 1;
            int yCount = (sizeY - 1) / splitY + 1;
            int zCount = (sizeZ - 1) / splitZ + 1;
            for(int x = 0; x < xCount; x++)
            {
                int xFrom = x * splitX;
                int xTo = Math.Min(xFrom + splitX - 1, sizeX - 1);
                for(int y = 0; y < yCount; y++)
                {
                    int yFrom = y * splitY;
                    int yTo = Math.Min(yFrom + splitY - 1, sizeY - 1);
                    for(int z = 0; z < zCount; z++)
                    {
                        int zFrom = z * splitZ;
                        MergeNeighborsInternal(cuboids, xFrom, yFrom, zFrom, xTo, yTo, Math.Min(zFrom + splitZ - 1, sizeZ - 1));
                    }
                }
            }
        }

        private static void MergeNeighborsInternal(MaterialCuboidInfo[,,] cuboids, int fromX, int fromY, int fromZ, int toX, int toY, int toZ)
        {
            for(int x = fromX; x <= toX; x++)
            {
                for(int y = fromY; y <= toY; y++)
                {
                    int index = fromZ;
                    while(index < toZ)
                    {
                        var baseCuboid = cuboids[x, y, index];
                        var nextCuboid = cuboids[x, y, index + 1];
                        if(baseCuboid != null && nextCuboid != null && baseCuboid.material == nextCuboid.material)
                        {
                            if((baseCuboid.neighbors & IGONORE_Z_MASK) == (nextCuboid.neighbors & IGONORE_Z_MASK))
                            {
                                cuboids[x, y, index + 1] = baseCuboid;
                                baseCuboid.sizeZ++;
                                baseCuboid.neighbors = (baseCuboid.neighbors & (~(1 << 4))) | (nextCuboid.neighbors & (1 << 4));
                            }
                        }
                        index++;
                    }
                }
            }
            for(int z = fromZ; z <= toZ; z++)
            {
                for(int y = fromY; y <= toY; y++)
                {
                    int index = fromX;
                    while(index < toX)
                    {
                        var baseCuboid = cuboids[index, y, z];
                        if(baseCuboid != null)
                        {
                            if(baseCuboid.movedTo == null)
                            {
                                var nextCuboid = cuboids[index + 1, y, z];
                                if(nextCuboid != null && baseCuboid != nextCuboid && baseCuboid != nextCuboid.movedTo && baseCuboid.material == nextCuboid.material)
                                {
                                    if((baseCuboid.neighbors & IGONORE_X_MASK) == (nextCuboid.neighbors & IGONORE_X_MASK))
                                    {
                                        if(baseCuboid.sizeY == nextCuboid.sizeY && baseCuboid.sizeZ == nextCuboid.sizeZ &&
                                            baseCuboid.y == nextCuboid.y && baseCuboid.z == nextCuboid.z)
                                        {
                                            nextCuboid.movedTo = baseCuboid;
                                            cuboids[index + 1, y, z] = baseCuboid;
                                            baseCuboid.sizeX++;
                                            baseCuboid.neighbors = (baseCuboid.neighbors & (~1)) | (nextCuboid.neighbors & 1);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                cuboids[index, y, z] = (MaterialCuboidInfo)baseCuboid.movedTo;
                            }
                        }
                        index++;
                    }
                    var cuboid = cuboids[toX, y, z];
                    if(cuboid != null && cuboid.movedTo != null)
                    {
                        cuboids[toX, y, z] = (MaterialCuboidInfo)cuboid.movedTo;
                    }
                }
            }
            for(int z = fromZ; z <= toZ; z++)
            {
                for(int x = fromX; x <= toX; x++)
                {
                    int index = fromY;
                    while(index < toY)
                    {
                        var baseCuboid = cuboids[x, index, z];
                        if(baseCuboid != null)
                        {
                            if(baseCuboid.movedTo == null)
                            {
                                var nextCuboid = cuboids[x, index + 1, z];
                                if(nextCuboid != null && baseCuboid != nextCuboid && baseCuboid != nextCuboid.movedTo && baseCuboid.material == nextCuboid.material)
                                {
                                    if((baseCuboid.neighbors & IGONORE_Y_MASK) == (nextCuboid.neighbors & IGONORE_Y_MASK))
                                    {
                                        if(baseCuboid.sizeX == nextCuboid.sizeX && baseCuboid.sizeZ == nextCuboid.sizeZ &&
                                            baseCuboid.x == nextCuboid.x && baseCuboid.z == nextCuboid.z)
                                        {
                                            nextCuboid.movedTo = baseCuboid;
                                            cuboids[x, index + 1, z] = baseCuboid;
                                            baseCuboid.sizeY++;
                                            baseCuboid.neighbors = (baseCuboid.neighbors & (~(1 << 2))) | (nextCuboid.neighbors & (1 << 2));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                cuboids[x, index, z] = (MaterialCuboidInfo)baseCuboid.movedTo;
                            }
                        }
                        index++;
                    }
                    var cuboid = cuboids[x, toY, z];
                    if(cuboid != null && cuboid.movedTo != null)
                    {
                        cuboids[x, toY, z] = (MaterialCuboidInfo)cuboid.movedTo;
                    }
                }
            }
        }

        private static bool CullSide(int main, int neighbor, IReadOnlyList<MaterialProperties> materials, bool cullBetweenTransparents)
        {
            if(main == neighbor) return true;
            if(neighbor < 0) return false;

            bool mainOpaque = true;
            switch((EnumChunkRenderPass)materials[main].renderPass)
            {
                case EnumChunkRenderPass.Liquid:
                case EnumChunkRenderPass.OpaqueNoCull:
                case EnumChunkRenderPass.BlendNoCull:
                    return false;
                case EnumChunkRenderPass.Meta:
                case EnumChunkRenderPass.TopSoil:
                case EnumChunkRenderPass.Transparent:
                    mainOpaque = false;
                    break;
            }
            bool neighborOpaque = true;
            switch((EnumChunkRenderPass)materials[neighbor].renderPass)
            {
                case EnumChunkRenderPass.Meta:
                case EnumChunkRenderPass.TopSoil:
                case EnumChunkRenderPass.Transparent:
                case EnumChunkRenderPass.BlendNoCull:
                    neighborOpaque = false;
                    break;
            }
            if(mainOpaque & neighborOpaque)
            {
                return true;
            }
            else
            {
                if(mainOpaque) return false;
                return neighborOpaque | cullBetweenTransparents;
            }
        }
    }
}