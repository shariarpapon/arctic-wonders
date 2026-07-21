using UnityEngine;

namespace Arctic.Foundation.Editor.Tabs
{
    public sealed class Tab
    {
        public readonly string Name;
        public readonly System.Action Draw;
        public Tab(string name, System.Action draw)
        {
            Name = name;
            this.Draw = draw;
        }
    }
}