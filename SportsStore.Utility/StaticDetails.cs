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
        public const string Role_Customer = "Покупатель";
        public const string Role_Admin = "Администратор";
        // Статусы заказа
        public const string StatusPending = "В ожидании";
        public const string StatusApproved = "Подтвреждён";
        public const string StatusInProcess = "В процессе";
        public const string StatusShipped = "Отгружен";
        public const string StatusCancelled = "Отменён";
        public const string StatusRefunded = "Возврат средств";
        // Статусы оплаты
        public const string PaymentStatusPending = "В ожидании";
        public const string PaymentStatusApproved = "Подтвреждён";
        public const string PaymentStatusRejected = "Отклонён";

    }
}
