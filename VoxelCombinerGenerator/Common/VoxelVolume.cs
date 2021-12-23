using System;

namespace VoxelCombinerGenerator
{
    public class VoxelVolume
    {
        public int x, y, z;
        public int sizeX, sizeY, sizeZ;
        public int[,,] voxels;

        public VoxelVolume(int x, int y, int z, int[,,] voxels)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.voxels = voxels;
            this.sizeX = voxels.GetLength(0);
            this.sizeY = voxels.GetLength(1);
            this.sizeZ = voxels.GetLength(2);
        }

        public void EnsureVolume(VoxelVolume other)
        {
            int minX = Math.Min(x, other.x);
            int minY = Math.Min(y, other.y);
            int minZ = Math.Min(z, other.z);
            int maxX = Math.Max(x + sizeX, other.x + other.sizeX);
            int maxY = Math.Max(y + sizeY, other.y + other.sizeY);
            int maxZ = Math.Max(z + sizeZ, other.z + other.sizeZ);
            int sx = maxX - minX;
            int sy = maxY - minY;
            int sz = maxZ - minZ;
            if(sx != sizeX || sy != sizeY || sz != sizeZ)
            {
                var newVoxels = new int[sx, sy, sz];
                unsafe
                {
                    int length = sx * sy * sz;
                    fixed(int* ptr = newVoxels)
                    {
                        for(int i = 0; i < length; i++)
                        {
                            ptr[i] = -1;
                        }
                    }
                    if(sizeX > 0 && sizeY > 0 && sizeZ > 0)
                    {
                        fixed(int* newPtr = newVoxels, oldPtr = voxels)
                        {
                            int startX = x - minX;
                            int startY = y - minY;
                            int startZ = z - minZ;
                            int oldSyz = sizeY * sizeZ;
                            int newSyz = sy * sz;
                            for(int x = 0; x < sizeX; x++)
                            {
                                int oldOffsetX = x * oldSyz;
                                int newOffsetX = (startX + x) * newSyz;
                                for(int y = 0; y < sizeY; y++)
                                {
                                    int oldOffsetY = oldOffsetX + y * sizeZ;
                                    int newOffsetY = newOffsetX + (startY + y) * sz;
                                    for(int z = 0; z < sizeZ; z++)
                                    {
                                        newPtr[newOffsetY + startZ + z] = oldPtr[oldOffsetY + z];
                                    }
                                }
                            }
                        }
                    }
                }
                this.voxels = newVoxels;
                this.sizeX = sx;
                this.sizeY = sy;
                this.sizeZ = sz;
            }
            this.x = minX;
            this.y = minY;
            this.z = minZ;
        }

        public bool InBounds(int x, int y, int z)
        {
            if(x < this.x) return false;
            if(y < this.y) return false;
            if(z < this.z) return false;
            x -= this.x;
            if(x >= this.sizeX) return false;
            y -= this.y;
            if(y >= this.sizeY) return false;
            z -= this.z;
            return z < this.sizeZ;
        }

        /// <summary>
        /// Computes the boundaries of <paramref name="other"/> <see cref="voxels"/> array that intersect with this volume.
        /// </summary>
        public bool TryGetLoopLimits(VoxelVolume other, out int startX, out int startY, out int startZ, out int endX, out int endY, out int endZ)
        {
            startX = Math.Max(other.x, x) - other.x;
            endX = Math.Min(other.x + other.sizeX, x + sizeX) - other.x;
            if(endX <= startX)
            {
                startY = 0;
                startZ = 0;
                endY = 0;
                endZ = 0;
                return false;
            }
            startY = Math.Max(other.y, y) - other.y;
            endY = Math.Min(other.y + other.sizeY, y + sizeY) - other.y;
            if(endY <= startY)
            {
                startZ = 0;
                endZ = 0;
                return false;
            }
            startZ = Math.Max(other.z, z) - other.z;
            endZ = Math.Min(other.z + other.sizeZ, z + sizeZ) - other.z;
            return endZ > startZ;
        }
    }
}