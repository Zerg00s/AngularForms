using SPMeta2.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Console.Models
{
    public static class SecurityGroupsDefinition
    {
        
        public static SecurityGroupDefinition SGTAtArms = new SecurityGroupDefinition
        {

            Name = "SGT-at-Arms",       

        };
        public static SecurityGroupDefinition WAM = new SecurityGroupDefinition
        {

            Name = "WAM",         

        };
        
        public static SecurityGroupDefinition MajorityCaucusLeader = new SecurityGroupDefinition
        {

            Name = "Majority Caucus Leader",          

        };
        public static SecurityGroupDefinition MinorityCaucusLeader = new SecurityGroupDefinition
        {

            Name = "Minority Caucus Leader",
           

        };
        public static SecurityGroupDefinition TravelRequestClerk = new SecurityGroupDefinition
        {

            Name = "Travel Request Clerk",
          
        };
        public static SecurityGroupDefinition Accounting = new SecurityGroupDefinition
        {

            Name = "Accounting",
          

        };
        public static SecurityGroupDefinition SenatePresident = new SecurityGroupDefinition
        {

            Name = "Senate President",          

        };

    }
}
