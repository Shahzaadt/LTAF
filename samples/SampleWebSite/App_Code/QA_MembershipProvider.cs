using System;
using System.Web.Security;

namespace Mvc.QA
{
    public class StandAloneMembershipProvider : SqlMembershipProvider
    {
        public StandAloneMembershipProvider()
        {
        }

        public override bool ValidateUser(string name, string password)
        {
            if (name.StartsWith("Valid"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object userId, out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.Success;
            MembershipUser user = new MembershipUser(
                this.Name, 
                username, 
                null, 
                email, 
                passwordQuestion, 
                "no comment", 
                true, 
                false, 
                DateTime.Now, 
                DateTime.Now, 
                DateTime.Now, 
                DateTime.Now, 
                DateTime.Now);

            return user;
        }
    }
}