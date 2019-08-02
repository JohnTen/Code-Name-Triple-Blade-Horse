namespace TripleBladeHorse.Animation
{
    public class AnimationEventArg : System.EventArgs
    {
        public AnimationState _state;
        public Animation _animation;

        public AnimationEventArg() { }
        public AnimationEventArg(AnimationState state, Animation animation)
        {
            _state = state;
            _animation = animation;
        }
    }

    public class FrameEventEventArg : System.EventArgs
    {
        public string _name;
        public float _floatData;
        public bool _boolData;
        public int _intData;
        public Animation _animation;


        public FrameEventEventArg() { }
        public FrameEventEventArg(string name, float floatData, bool boolData, int intData, Animation animation)
        {
            _name = name;
            _floatData = floatData;
            _boolData = boolData;
            _intData = intData;
            _animation = animation;
        }
    }
}
