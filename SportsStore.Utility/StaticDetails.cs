using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Utility
{
    public static class StaticDetails
    {
        // Роли
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";
        // Статусы заказа
        public const string StatusPending = "В ожидании";
        public const string StatusApproved = "Подтвреждён";
        public const string StatusInProcess = "В процессе";
        public const string StatusShipped = "Отгружен";
        public const string StatusCancelled = "Отменён";
        public const string StatusRefunded = "Возврат средств";

        // Статус оплаты
        public const string PaymentStatusApproved = "Оплата подтверждена";

        public const string SessionCart = "SessionShoppingCart";

    }
}
