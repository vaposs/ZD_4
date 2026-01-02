using System;
using System.Security.Cryptography;
using System.Text;

namespace ZD_4
{
    class Program
    {
        static void Main(string[] args)
        {
            string key = "qwerty";

            Order order = new Order(0001, 1000);

            IHashSystem hashSystemM = new HashSystemMD5();
            IHashSystem hashSystemS = new HashSystemSha1();

            PaymentSystemFirst paymentSystemFirst = new PaymentSystemFirst(hashSystemM);
            PaymentSystemSecond paymentSystemSecond = new PaymentSystemSecond(hashSystemM);
            PaymentSystemThird paymentSystemThird = new PaymentSystemThird(key, hashSystemS);

            Console.WriteLine(paymentSystemFirst.GetPayingLink(order));
            Console.WriteLine(paymentSystemSecond.GetPayingLink(order));
            Console.WriteLine(paymentSystemThird.GetPayingLink(order));
        }
    }

    public class Order
    {
        public Order(int id, int amount)
        {
            Id = id;
            Amount = amount;
        }

        public int Id { get; private set; }
        public int Amount { get; private set; }
    }

    public interface IPaymentSystem
    {
        string GetPayingLink(Order order);
    }

    public interface IHashSystem
    {
        string CalculateHash(string input);
    }

    public class HashSystemMD5 : IHashSystem
    {
        public string CalculateHash(string line)
        {
            string codingLine = "x2";
            MD5 mD5 = MD5.Create();
            StringBuilder stringBuilder = new StringBuilder();

            if (mD5 != null)
            {
                byte[] inputLine = Encoding.UTF8.GetBytes(line);
                byte[] hashLine = mD5.ComputeHash(inputLine);

                for (int i = 0; i < hashLine.Length; i++)
                {
                    stringBuilder.Append(hashLine[i].ToString(codingLine));
                }
            }

            return stringBuilder.ToString();
        }
    }

    public class HashSystemSha1 : IHashSystem
    {
        public string CalculateHash(string line)
        {
            string codingLine = "x2";
            SHA1 sha = SHA1.Create();
            StringBuilder stringBuilder = new StringBuilder();

            if (sha != null)
            {
                byte[] inputLine = Encoding.UTF8.GetBytes(line);
                byte[] hashLine = sha.ComputeHash(inputLine);

                for (int i = 0; i < hashLine.Length; i++)
                {
                    stringBuilder.Append(hashLine[i].ToString(codingLine));
                }
            }

            return stringBuilder.ToString();
        }
    }

    abstract class BasePaymentSystem
    {
        private IHashSystem _hashSystem;

        public BasePaymentSystem(IHashSystem hashSystem)
        {
            _hashSystem = hashSystem ?? throw new ArgumentNullException(nameof(hashSystem));
        }

        protected IHashSystem HashSystem => _hashSystem;
    }

    class PaymentSystemFirst : BasePaymentSystem, IPaymentSystem
    {
        public PaymentSystemFirst(IHashSystem hashSystem) : base(hashSystem)
        {

        }

        public string GetPayingLink(Order order)
        {
            return $"pay.system1.ru/order?amount={order.Amount}RUB&hash={HashSystem.CalculateHash(order.Id.ToString())}";
        }
    }

    class PaymentSystemSecond : BasePaymentSystem, IPaymentSystem
    {
        public PaymentSystemSecond(IHashSystem hashSystem) : base(hashSystem)
        {

        }

        public string GetPayingLink(Order order)
        {
            return $"order.system2.ru/pay?hash={HashSystem.CalculateHash(order.Id.ToString() + order.Amount.ToString())}";
        }
    }

    class PaymentSystemThird : BasePaymentSystem, IPaymentSystem
    {
        private string _key;

        public PaymentSystemThird(string key, IHashSystem hashSystem) : base(hashSystem)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public string GetPayingLink(Order order)
        {
            return $"system3.com/pay?amount={order.Amount}&curency=RUB&hash={HashSystem.CalculateHash(order.Amount.ToString() + order.Id.ToString() + _key)}";
        }
    }
}