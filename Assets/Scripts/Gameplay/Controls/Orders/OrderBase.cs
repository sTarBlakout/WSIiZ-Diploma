
namespace Gameplay.Controls.Orders
{
    public abstract class OrderBase
    {
        public OrderBase(OrderArgsBase args) { }
        public abstract void StartOrder();
    }
}
