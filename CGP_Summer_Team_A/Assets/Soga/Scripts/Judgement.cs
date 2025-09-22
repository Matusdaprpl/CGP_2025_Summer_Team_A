using System.Collections.Generic;
using System.Linq;

public class Judgement
{
    private Dictionary<string, int> CountTiles(List<Tile> hand)
    {
        var counts = new Dictionary<string, int>();
        foreach (var tile in hand)
        {
            string key = tile.ToString();
            if (counts.ContainsKey(key))
            {
                counts[key]++;
            }
            else
            {
                counts[key] = 1;
            }
        }
        return counts;
    }
}