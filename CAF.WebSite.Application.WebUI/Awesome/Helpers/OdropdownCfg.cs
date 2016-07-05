namespace AwesomeMvcDemo.Helpers
{
    public class OdropdownCfg
    {
        private readonly OdropdownTag tag = new OdropdownTag();

        public OdropdownCfg InLabel(string o)
        {
            tag.InLabel = o;
            return this;
        }

        public OdropdownCfg Caption(string o)
        {
            tag.Caption = o;
            return this;
        }

        public OdropdownCfg AutoSelectFirst()
        {
            tag.AutoSelectFirst = true;
            return this;
        }

        public OdropdownCfg NoSelectClose()
        {
            tag.NoSelClose = true;
            return this;
        }

        internal OdropdownTag ToTag()
        {
            return tag;
        }
    }
}