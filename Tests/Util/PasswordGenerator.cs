using System.Collections.Generic;
using System.Linq;
using dynamics365accelerator.Model.Data;
using dynamics365accelerator.Tests.DataProviders;
using NUnit.Framework;
using Newtonsoft.Json;
using System.IO;
using System;
using dynamics365accelerator.Support.Utils;


namespace dynamics365accelerator.Tests.Util
{
    public class PasswordGenerator
    {
        //<Summary>
        //Utility to encrypt a clear text password for a given user and writes the emcrypted password to usercontextData.json files.
        //if the encryption is successful
        //
        //NoteL This test is only ever intended to be run locally for explicit purpose of updating the encrypted password field.
        //in file UserContextData.json
        //<Summary>

        [Test]
        [Ignore("To be run when necessary - Uncomment this line to run (be sure to re-comment if before committing)")]
        public void GeneratePassword()
        {

            var config = EnvConfig.Get();

            if (string.IsNullOrEmpty(config.GetSection("FB_PASSWORD_TO_ENCRYPT").Value))
                Assert.Fail("Environment variable 'FB_PASSWORD_TO_ENCRYPT' is missing or empty - this is the plain text password for encryption.");
            
            if (string.IsNullOrEmpty(config.GetSection("FB_ENCRYPT_PWD_FOR_USER").Value))
                Assert.Fail("Environment variable 'FB_ENCRYPT_PWD_FOR_USER' is missing or empty - this is the user's 'PrincipalName' for whome we are encrypting the password.");
            
            string userPrincipalName = config.GetSection("FB_ENCRYPT_PWD_FOR_USER").Value;
            string encryptedPassword = Cryptography.EncryptString(config.GetSection("FB_PASSWORD_TO_ENCRYPT").Value);

            Assert.That(config.GetSection("FB_PASSWORD_TO_ENCRYPT").Value.Equals(Cryptography.DecryptString(encryptedPassword)));
        
            UpdateJsonFileWithEncryptedPassword(userPrincipalName, encryptedPassword);       
        
        }

        //<summary>
        //Decrypt a password stored in environment variable 'FB_PASSWORD_TO_ENCRYPT';
        //This exists as a helper for when the scheduled password change request happens.
        //It requires the current password to be entered, and it's not straightforward to decrypt it without writing code like the code below
        //By design, it' expected that the debugger will be used to examine the password variable, rather than printing or logging the decrypted password.
        //<summary>

        [Test]
        [Ignore("To be run when necessary - Uncomment this line to run (be sure to re-comment if before committing)")]

        public void DecryptPassword()
        {
            
            var password = Cryptography.DecryptString(EnvConfig.Get().GetSection("FB_PASSWORD_TO_ENCRYPT").Value);
        }

        private void UpdateJsonFileWithEncryptedPassword(string userPrincipalName, string encryptedPassword)
        {

            List<UserContextData> userContextData = UserContextDataProvider.ReadUserData();

            Assert.That(userContextData.Where(user => user.PrincipalName.ToLower() == userPrincipalName.ToLower()).Any(), $"PrincipalUser '{userPrincipalName}' not found in UserContextData.json file.");

            try
            {
                UserContextData user = (userContextData.First(user => user.PrincipalName.ToLower() == userPrincipalName.ToLower()));
                user.EncryptedPassword = encryptedPassword;

                string updatedUserContextDataJson = JsonConvert.SerializeObject(userContextData, Formatting.Indented);
                string currentDirectory = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(currentDirectory).Parent.Parent.FullName;
                File.WriteAllText(Path.Combine(projectDirectory, @"Tests\DataProviders\UserContextData.json"), updatedUserContextDataJson);                    
            }

            catch(Exception e)
            {
                Assert.Fail("ERROR: UserContextData.json not updated with the encrypted password.\n"
                    + e.Message + "\n"
                    + e.StackTrace);

            }

        }

    }
    
}
