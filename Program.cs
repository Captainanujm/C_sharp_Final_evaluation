using System;

namespace HospitalManagementSystem
{
    public delegate double BillingStrategy(double amount);

    public abstract class Patient
    {
        public int PatientId { get; }
        public string Name { get; }
        public double BaseCharge { get; }

        protected Patient(int id, string name, double baseCharge)
        {
            PatientId = id;
            Name = name;
            BaseCharge = baseCharge;
        }

        public abstract double CalculateBill();
    }

    public class GeneralPatient : Patient
    {
        public GeneralPatient(int id, string name)
            : base(id, name, 2000) { }

        public override double CalculateBill()
        {
            return BaseCharge;
        }
    }

    public class EmergencyPatient : Patient
    {
        public EmergencyPatient(int id, string name)
            : base(id, name, 5000) { }

        public override double CalculateBill()
        {
            return BaseCharge + 1500;
        }
    }

    public class InsurancePatient : Patient
    {
        public InsurancePatient(int id, string name)
            : base(id, name, 3000) { }

        public override double CalculateBill()
        {
            return BaseCharge;
        }
    }

    public class HospitalNotifier
    {
        public event Action<string>? Notify;

        public void Trigger(string message)
        {
            Notify?.Invoke(message);
        }
    }

    public class BillingService
    {
        public static double ApplyStrategy(double amount, BillingStrategy strategy)
        {
            return strategy(amount);
        }

        public static double NormalBilling(double amount)
        {
            return amount;
        }

        public static double InsuranceDiscount(double amount)
        {
            return amount * 0.7;
        }
    }

    class Program
    {
        static void Main()
        {
            HospitalNotifier notifier = new HospitalNotifier();

            notifier.Notify += msg => Console.WriteLine("ADMIN ALERT: " + msg);
            notifier.Notify += msg => Console.WriteLine("BILLING DEPT: " + msg);
            notifier.Notify += msg => Console.WriteLine("MEDICAL TEAM: " + msg);

            Console.WriteLine("Enter Patient ID:");
            int id = int.Parse(Console.ReadLine()!);

            Console.WriteLine("Enter Patient Name:");
            string name = Console.ReadLine()!;

            Console.WriteLine("Select Patient Type");
            Console.WriteLine("1. General");
            Console.WriteLine("2. Emergency");
            Console.WriteLine("3. Insurance");

            int choice = int.Parse(Console.ReadLine()!);

            Patient patient;
            BillingStrategy strategy;

            switch (choice)
            {
                case 1:
                    patient = new GeneralPatient(id, name);
                    strategy = BillingService.NormalBilling;
                    break;

                case 2:
                    patient = new EmergencyPatient(id, name);
                    strategy = BillingService.NormalBilling;
                    break;

                case 3:
                    patient = new InsurancePatient(id, name);
                    strategy = BillingService.InsuranceDiscount;
                    break;

                default:
                    Console.WriteLine("Invalid option");
                    return;
            }

            double baseBill = patient.CalculateBill();
            double finalBill = BillingService.ApplyStrategy(baseBill, strategy);

            Console.WriteLine("\n----- BILL DETAILS -----");
            Console.WriteLine($"Patient Name: {patient.Name}");
            Console.WriteLine($"Patient Type: {patient.GetType().Name}");
            Console.WriteLine($"Final Bill Amount: ₹{finalBill}");

            notifier.Trigger($"Patient {patient.Name} admitted with bill ₹{finalBill}");

            Console.WriteLine("\nProcess Completed Successfully");
        }
    }
}
