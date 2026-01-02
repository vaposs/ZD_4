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

            PaymentSystemFirst paymentSystemFirst = new PaymentSystemFirst();
            PaymentSystemThird paymentSystemThird = new PaymentSystemThird(key);
            PaymentSystemSecond paymentSystemSecond = new PaymentSystemSecond();

            Console.WriteLine(paymentSystemFirst.GetPayingLink(order));
            Console.WriteLine(paymentSystemSecond.GetPayingLink(order));
            Console.WriteLine(paymentSystemThird.GetPayingLink(order));
        }
    }

    public class Order
    {
        public int Id { get; private set; }
        public int Amount { get; private set; }

        public Order(int id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }

    public interface IPaymentSystem
    {
        string GetPayingLink(Order order);
    }

    abstract class BasePaymentSystem
    {
        private byte[] _inputLine;
        private byte[] _hashLine;
        private StringBuilder _stringBuilder = new StringBuilder();
        private string _codingLine = "x2";

        protected string CalculateMD5(string line)
        {
            MD5 mD5 = MD5.Create();
            _stringBuilder.Clear();

            if (mD5 != null)
            {
                _inputLine = Encoding.UTF8.GetBytes(line);
                _hashLine = mD5.ComputeHash(_inputLine);

                for (int i = 0; i < _hashLine.Length; i++)
                {
                    _stringBuilder.Append(_hashLine[i].ToString(_codingLine));
                }
            }

            return _stringBuilder.ToString();
        }

        protected string CalculateSha1(string line)
        {
            SHA1 sha = SHA1.Create();
            _stringBuilder.Clear();

            if (sha != null)
            {
                _inputLine = Encoding.UTF8.GetBytes(line);
                _hashLine = sha.ComputeHash(_inputLine);

                for (int i = 0; i < _hashLine.Length; i++)
                {
                    _stringBuilder.Append(_hashLine[i].ToString(_codingLine));
                }
            }

            return _stringBuilder.ToString();
        }
    }

    class PaymentSystemFirst : BasePaymentSystem, IPaymentSystem
    {
        public string GetPayingLink(Order order)
        {
            return $"pay.system1.ru/order?amount={order.Amount}RUB&hash={CalculateMD5(order.Id.ToString())}";
        }
    }

    class PaymentSystemSecond : BasePaymentSystem, IPaymentSystem
    {
        public string GetPayingLink(Order order)
        {
            return $"order.system2.ru/pay?hash={CalculateMD5(order.Id.ToString() + order.Amount.ToString())}";
        }
    }

    class PaymentSystemThird : BasePaymentSystem, IPaymentSystem
    {
        private string _key;

        public PaymentSystemThird(string key)
        {
            _key = key;
        }

        public string GetPayingLink(Order order)
        {
            return $"system3.com/pay?amount={order.Amount}&curency=RUB&hash={CalculateSha1(order.Amount.ToString() + order.Id.ToString() + _key)}";
        }
    }
}
