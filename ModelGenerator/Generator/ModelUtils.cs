using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ModelGenerator
{
    public static class ModelUtils
    {
        private const int IGONORE_X_MASK = ~(1 | (1 << 1));
        private const int IGONORE_Y_MASK = ~((1 << 2) | (1 << 3));
        private const int IGONORE_Z_MASK = ~((1 << 4) | (1 << 5));
        private const int ALL_NEIGHBORS = 1 | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5);

        public static CuboidInfo[,,] VoxelsToCuboids(bool[,,] voxels, int sizeX, int sizeY, int sizeZ)
        {
            var cuboids = new CuboidInfo[sizeX, sizeY, sizeZ];
            sizeX--;
            sizeY--;
            sizeZ--;
            for(int x = 0; x <= sizeX; x++)
            {
                for(int y = 0; y <= sizeY; y++)
                {
                    for(int z = 0; z <= sizeZ; z++)
                    {
                        if(voxels[x, y, z])
                        {
                            int neighbors = 0;
                            if(x < sizeX && voxels[x + 1, y, z]) neighbors |= 1;
                            if(x > 0 && voxels[x - 1, y, z]) neighbors |= 1 << 1;
                            if(y < sizeY && voxels[x, y + 1, z]) neighbors |= 1 << 2;
                            if(y > 0 && voxels[x, y - 1, z]) neighbors |= 1 << 3;
                            if(z < sizeZ && voxels[x, y, z + 1]) neighbors |= 1 << 4;
                            if(z > 0 && voxels[x, y, z - 1]) neighbors |= 1 << 5;

                            var cuboid = new CuboidInfo(x, y, z, 1, 1, 1);
                            cuboid.neighbors = neighbors;
                            cuboids[x, y, z] = cuboid;
                        }
                    }
                }
            }
            return cuboids;
        }

        /// <summary>
        /// Removes voxels that have all neighbors
        /// </summary>
        public static void RemoveInvisibleCuboids(CuboidInfo[,,] cuboids, int sizeX, int sizeY, int sizeZ)
        {
            sizeX--;
            sizeY--;
            sizeZ--;
            for(int x = 0; x <= sizeX; x++)
            {
                for(int y = 0; y <= sizeY; y++)
                {
                    for(int z = 0; z <= sizeZ; z++)
                    {
                        if(cuboids[x, y, z] != null && cuboids[x, y, z].neighbors == ALL_NEIGHBORS)
                        {
                            cuboids[x, y, z] = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Combines adjacent voxels into a box
        /// </summary>
        public static void MergeNeighbors(CuboidInfo[,,] cuboids, int sizeX, int sizeY, int sizeZ)
        {
            sizeX--;
            sizeY--;
            sizeZ--;
            for(int x = 0; x <= sizeX; x++)
            {
                for(int y = 0; y <= sizeY; y++)
                {
                    int index = 0;
                    while(index < sizeZ)
                    {
                        var baseCuboid = cuboids[x, y, index];
                        var nextCuboid = cuboids[x, y, index + 1];
                        if(baseCuboid != null && nextCuboid != null)
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
            for(int z = 0; z <= sizeZ; z++)
            {
                for(int y = 0; y <= sizeY; y++)
                {
                    int index = 0;
                    while(index < sizeX)
                    {
                        var baseCuboid = cuboids[index, y, z];
                        if(baseCuboid != null)
                        {
                            if(baseCuboid.movedTo == null)
                            {
                                var nextCuboid = cuboids[index + 1, y, z];
                                if(nextCuboid != null && baseCuboid != nextCuboid && baseCuboid != nextCuboid.movedTo)
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
                                cuboids[index, y, z] = baseCuboid.movedTo;
                            }
                        }
                        index++;
                    }
                    var cuboid = cuboids[sizeX, y, z];
                    if(cuboid != null && cuboid.movedTo != null)
                    {
                        cuboids[sizeX, y, z] = cuboid.movedTo;
                    }
                }
            }
            for(int z = 0; z <= sizeZ; z++)
            {
                for(int x = 0; x <= sizeX; x++)
                {
                    int index = 0;
                    while(index < sizeY)
                    {
                        var baseCuboid = cuboids[x, index, z];
                        if(baseCuboid != null)
                        {
                            if(baseCuboid.movedTo == null)
                            {
                                var nextCuboid = cuboids[x, index + 1, z];
                                if(nextCuboid != null && baseCuboid != nextCuboid && baseCuboid != nextCuboid.movedTo)
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
                                cuboids[x, index, z] = baseCuboid.movedTo;
                            }
                        }
                        index++;
                    }
                    var cuboid = cuboids[x, sizeY, z];
                    if(cuboid != null && cuboid.movedTo != null)
                    {
                        cuboids[x, sizeY, z] = cuboid.movedTo;
                    }
                }
            }
        }

        public static ShapeElement CuboidToShapeElement(CuboidInfo cuboid, Vec3d offset, int textureWidth, int textureHeight)
        {
            return new ShapeElement() {
                From = new double[] { offset.X + cuboid.x, offset.Y + cuboid.y, offset.Z + cuboid.z },
                To = new double[] { offset.X + cuboid.x + cuboid.sizeX, offset.Y + cuboid.y + cuboid.sizeY, offset.Z + cuboid.z + cuboid.sizeZ },
                Faces = new Dictionary<string, ShapeElementFace>() {
                    { BlockFacing.EAST.Code, CreateFace(cuboid, 0, textureWidth, textureHeight) },
                    { BlockFacing.WEST.Code, CreateFace(cuboid, 1, textureWidth, textureHeight) },
                    { BlockFacing.UP.Code, CreateFace(cuboid, 2, textureWidth, textureHeight) },
                    { BlockFacing.DOWN.Code, CreateFace(cuboid, 3, textureWidth, textureHeight) },
                    { BlockFacing.SOUTH.Code, CreateFace(cuboid, 4, textureWidth, textureHeight) },
                    { BlockFacing.NORTH.Code, CreateFace(cuboid, 5, textureWidth, textureHeight) }
                }
            };
        }

        private static ShapeElementFace CreateFace(CuboidInfo cuboid, int face, int width, int height)
        {
            int u = cuboid.sizeX;
            int uo = cuboid.x;
            int v = cuboid.sizeY;
            int vo = cuboid.y;
            switch(face)
            {
                case 0:
                case 1:
                    u = cuboid.sizeZ;
                    uo = cuboid.z;
                    break;
                case 2:
                case 3:
                    v = cuboid.sizeZ;
                    vo = cuboid.z;
                    break;
            }
            uo = uo % width;
            vo = vo % height;
            if(uo + u > width) uo = width - u;
            if(vo + v > height) vo = height - v;
            u += uo;
            v += vo;
            int t = vo;
            vo = height - v;
            v = height - t;
            if(face == 0 || face == 3 || face == 5)
            {
                u = width - u;
                uo = width - uo;
                t = u;
                u = uo;
                uo = t;
            }
            return new ShapeElementFace() {
                Uv = new float[] { uo, vo, u, v },
                Enabled = (cuboid.neighbors & (1 << face)) == 0
            };
        }
    }
}