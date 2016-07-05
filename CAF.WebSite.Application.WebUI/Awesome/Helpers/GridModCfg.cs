namespace AwesomeMvcDemo.Helpers
{
    public class GridModCfg
    {
        private readonly GridModInfo info = new GridModInfo();

        public GridModCfg InfiniteScroll()
        {
            info.InfiniteScroll = true;
            return this;
        }

        public GridModCfg PageInfo()
        {
            info.PageInfo = true;
            return this;
        }

        public GridModCfg AutoMiniPager()
        {
            info.AutoMiniPager = true;
            return this;
        }

        public GridModCfg PageSize()
        {
            info.PageSize = true;
            return this;
        }

        public GridModCfg ColumnsSelector()
        {
            info.ColumnsSelector = true;
            return this;
        }

        internal GridModInfo GetInfo()
        {
            return info;
        }
    }
}