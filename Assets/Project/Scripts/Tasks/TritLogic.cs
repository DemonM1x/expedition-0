using System;
using System.Runtime.CompilerServices;

namespace Expedition0.Tasks
{
    public static class TritLogic
    {
        // Conversions
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this Trit t) => (int)t;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trit FromInt(int v) => (Trit)(v <= 0 ? 0 : (v >= 2 ? 2 : 1));

        // Unary operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trit Not(this Trit t) => (Trit)(2 - (int)t);
        
        // Binary Operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trit And(this Trit left, Trit right) => (Trit)(Math.Min((int) left, (int) right));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trit Or(this Trit left, Trit right) => (Trit)(Math.Max((int) left, (int) right));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trit ImplyKleene(this Trit a, Trit b) => a.Not().Or(b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trit ImplyLukasiewicz(this Trit a, Trit b) =>
            (a == Trit.Neutral && b == Trit.Neutral) ? Trit.True : a.ImplyKleene(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trit Xor(this Trit a, Trit b) =>
            a.And(b.Not()).Or(a.Not().And(b));
        
        // Common derivatives
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trit Nand(this Trit a, Trit b) => a.And(b).Not();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trit Nor(this Trit a, Trit b)  => a.Or(b).Not();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Trit Equiv(this Trit a, Trit b) => a.Xor(b).Not();
        
        // N-ary operators
        public static Trit And(params Trit[] xs)
        {
            var r = Trit.True;
            for (int i = 0; i < xs.Length; i++) r = r.And(xs[i]);
            return r;
        }
        
        public static Trit Or(params Trit[] xs)
        {
            var r = Trit.False;
            for (int i = 0; i < xs.Length; i++) r = r.Or(xs[i]);
            return r;
        }
    }
}