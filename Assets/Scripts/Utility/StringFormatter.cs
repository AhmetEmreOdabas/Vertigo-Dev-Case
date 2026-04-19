namespace DevCase.Utility
{
    public static class StringFormatter
    {
        public static string ToShortString(int value)
        {
            if (value < 1000)
                return value.ToString();

            if (value < 1_000_000)
                return (value / 1000f).ToString("0.#") + "K";

            if (value < 1_000_000_000)
                return (value / 1_000_000f).ToString("0.#") + "M";

            return (value / 1_000_000_000f).ToString("0.#") + "B";
        }
    }
}
