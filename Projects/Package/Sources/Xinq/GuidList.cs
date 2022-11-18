using System;

namespace Xinq
{
    internal static class GuidList
    {
        public const string XinqPackageGuidString = "C2BD518B-C09A-401C-B9D0-93736786E488";
        public const string XinqEditorFactoryGuidString = "977D2DD7-0DFD-4B48-B6F5-329EE04FFB80";
        public const string XinqSingleFileGeneratorGuidString = "2252AA80-D1C4-4943-A0F2-BF9D75DC1096";
        public static readonly Guid XinqPackageGuid = new Guid(XinqPackageGuidString);
        public static readonly Guid XinqEditorFactoryGuid = new Guid(XinqEditorFactoryGuidString);
    };
}
