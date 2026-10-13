using Microsoft.AspNetCore.Http;

namespace raceday_management_system.Tests
{//start of namespace
    public static class SessionHelper
    {//start of class

        //makes a DefaultHttpContext with a fake session
        public static DefaultHttpContext make_context(string? role = null, int? user_id = null, string? email = null)
        {//start of make_context()

            var ctx = new DefaultHttpContext();
            ctx.Session = new FakeSession();

            if (role != null)
            {//start of if

                ctx.Session.SetString("Role", role);

            }//end of if

            if (user_id.HasValue)
            {//start of if

                ctx.Session.SetInt32("User_id", user_id.Value);

            }//end of if

            if (email != null)
            {//start of if

                ctx.Session.SetString("Email", email);

            }//end of if

            return ctx;

        }//end of make_context()

    }//end of class

}//end of namespace