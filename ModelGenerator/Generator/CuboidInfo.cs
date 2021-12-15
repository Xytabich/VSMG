namespace ModelGenerator
{
    public class CuboidInfo
    {
        public int x, y, z;
        public int sizeX, sizeY, sizeZ;
        public int neighbors = 0;

        public CuboidInfo movedTo = null;

        public CuboidInfo(int x, int y, int z, int sizeX, int sizeY, int sizeZ)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.sizeZ = sizeZ;
        }
    }
}