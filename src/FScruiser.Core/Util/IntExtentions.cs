using System.Collections.Generic;

namespace FScruiser.Util
{
    public static class IntExtentions
    {
        private const string ALPHABET = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";

        public static string ToAlphanumeric(this int @this, bool pad = false)
        {
            var stack = new Stack<char>();
            while (@this > 0)
            {
                stack.Push(ALPHABET[@this % ALPHABET.Length]);
                @this /= ALPHABET.Length;
            }

            var str = new string(stack.ToArray());
            if(pad) { str = str.PadLeft(6, '0'); }
            return str;
        }
    }
}