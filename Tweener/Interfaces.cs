namespace NilanToolkit.Tweener {
    
    public interface ITweener {
    
        bool IsActive { get; set; }
    
        /// <summary>
        /// begin point of motion, default 0
        /// </summary>
        float StartTime { get; }
    
        /// <summary>
        /// length of the tweener motion
        /// </summary>
        float Duration { get; }
    
        /// <summary>
        /// current time of motion timeline
        /// </summary>
        float CurrentTime { get; set; }

        /// <summary>
        /// callback when finish
        /// </summary>
        event TweenCallback OnComplete;
    
        /// <summary>
        /// setup tweener to default status, call when motion start
        /// </summary>
        void Setup();
    
        /// <summary>
        /// calculate a value by CurrentTime and Duration, and set to owner
        /// </summary>
        void Update();

        /// <summary>
        /// complete the tweener
        /// </summary>
        void Complete();

        /// <summary>
        /// set motion to final status
        /// </summary>
        void FastForward();
    
    }
    
    public interface ITweenCollection : ITweener {
    
        int Count { get; }
    
        /// <summary>
        /// add a item to collection
        /// </summary>
        /// <param name="item"></param>
        void Add(ITweener item);
    
        ITweener this[int index] { get; }
    
    }
    
    public interface ITweenSequence : ITweenCollection {
        /// <summary>
        /// the last item of sequence
        /// </summary>
        ITweener Tail { get; }
    }

    public interface IRelativeableTweener {
        bool IsRelative { get; set; }
    }

    
}
