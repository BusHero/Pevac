namespace Pevac
{
    public sealed class None
    {
        public static None Instance { get; } = new None();

        private None() { }
    }
}