using Gameplay.Environment;

namespace Gameplay.Controls.Orders
{
    public class CompleteOrderArgsMove : CompleteOrderArgsBase
    {
        public GameAreaTile MovedToTile { get; set; } = null;
    }
}
