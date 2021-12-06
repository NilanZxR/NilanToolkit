namespace NilanToolkit.DirtyMark {

    public delegate void RefreshFunction();
    
    public class DirtyMarkHandler : IDirtyMarkable {
        
        public RefreshFunction refreshFunction;

        public DirtyMarkHandler(RefreshFunction func) {
            refreshFunction = func;
        }
        
        public void OnDirtyStateRefresh() {
            refreshFunction?.Invoke();
        }
    }
}