using Gameplay.Core;

namespace Gameplay.Controls.Orders
{
    public abstract class CompleteOrderArgsBase
    {
        public OrderResult Result { get; set; } = OrderResult.NotDetermined;
        public OrderFailReason FailReason { get; set; } = OrderFailReason.None;
    }
}
