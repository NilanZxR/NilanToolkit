namespace NilanToolkit.Selection {
    public interface ISelection {
    
        /// <summary>
        /// don't change this value by hand, use Selection.Select()
        /// </summary>
        bool IsSelect { get; set; }

        void OnSelect();

        void OnDeselect();

    }
}