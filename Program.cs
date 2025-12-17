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
            : base(id, name, 2000)
        {
        }

        public override double CalculateBill()
        {
            return BaseCharge;
        }
    }

  
    public class EmergencyPatient : Patient
    {
        public EmergencyPatient(int id, string name)
            : base(id, name, 5000)
        {
        }

        public override double CalculateBill()
        {
            return BaseCharge + 1500;
        }
    }

   
    public class InsurancePatient : Patient
    {
        public InsurancePatient(int id, string name)
            : base(id, name, 3000)
        {
        }

        public override double CalculateBill()
        {
            return BaseCharge;
        }
    }

   
    public class HospitalNotifier
    {
        public event Action<string>? Notify;

        public void TriggerNotification(string message)
        {
            Notify?.Invoke(message);
        }
    }

   
    public class BillingService
    {
        public static double ApplyBilling(double amount, BillingStrategy strategy)
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

            notifier.Notify += message => Console.WriteLine("ADMIN ALERT: " + message);
            notifier.Notify += message => Console.WriteLine("BILLING DEPARTMENT: " + message);
            notifier.Notify += message => Console.WriteLine("MEDICAL TEAM: " + message);

            Console.WriteLine("Enter Patient ID:");
            int patientId = int.Parse(Console.ReadLine()!);

            Console.WriteLine("Enter Patient Name:");
            string patientName = Console.ReadLine()!;

            Console.WriteLine("\nSelect Patient Type:");
            Console.WriteLine("1. General Patient");
            Console.WriteLine("2. Emergency Patient");
            Console.WriteLine("3. Insurance Patient");

            int choice = int.Parse(Console.ReadLine()!);

            Patient patient;
            BillingStrategy strategy;

            switch (choice)
            {
                case 1:
                    patient = new GeneralPatient(patientId, patientName);
                    strategy = BillingService.NormalBilling;
                    break;

                case 2:
                    patient = new EmergencyPatient(patientId, patientName);
                    strategy = BillingService.NormalBilling;
                    break;

                case 3:
                    patient = new InsurancePatient(patientId, patientName);
                    strategy = BillingService.InsuranceDiscount;
                    break;

                default:
                    Console.WriteLine("Invalid patient type selected.");
                    return;
            }

            double baseBill = patient.CalculateBill();
            double finalBill = BillingService.ApplyBilling(baseBill, strategy);

            Console.WriteLine("\n----- BILL DETAILS -----");
            Console.WriteLine("Patient ID   : " + patient.PatientId);
            Console.WriteLine("Patient Name : " + patient.Name);
            Console.WriteLine("Patient Type : " + patient.GetType().Name);
            Console.WriteLine("Final Bill   : ₹" + finalBill);

            notifier.TriggerNotification(
                $"Patient {patient.Name} admitted. Final bill amount ₹{finalBill}"
            );

            Console.WriteLine("\nProcess Completed Successfully.");
        }
    }
}
