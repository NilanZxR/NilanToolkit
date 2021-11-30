namespace NilanToolkit.DirtyMark {
    
    public static class Extensions {
        
        public static void SetDirty(this IDirtyMarkable target) {
            DirtyMark.SetDirty(target);
        }

        public static void SubscribeDirtyMarkChannel(this IDirtyMarkable target, string channelName) {
            DirtyMark.Subscribe(channelName, target);
        }
        
    }
    
}