using Microsoft.AspNetCore.Http;
using SixtyThreeBits.Core.Libraries.Extensions;
using SixtyThreeBits.Libraries.Extensions;
using System.Linq;

namespace SixtyThreeBits.Web.Domain.Libraries
{
    public class SessionAssistance63
    {
        #region Properties
        readonly ISession _session;
        const string SessionKeyUser = "User63";
        #endregion

        #region Constructors
        public SessionAssistance63(ISession session)
        {
            _session = session;
        }
        #endregion

        #region Methods
        public void Clear()
        {
            _session.Clear();
        }

        public T Get<T>(string key)
        {
            return HasKey(key) ? _session.GetString(key).DeserializeJsonTo<T>() : default;
        }

        public string GetSessionID()
        {
            return _session.Id;
        }

        public T GetUser<T>()
        {
            var userJsonEncrypted = _session.GetString(SessionKeyUser);
            var user = userJsonEncrypted.Decrypt<T>();
            return user;
        }

        public bool HasKey(string key)
        {
            return _session.Keys.Contains(key);
        }

        public void Set<T>(string key, T value)
        {
            _session.SetString(key, value.ToJson());
        }

        public void SetUser<T>(T user)
        {
            if (user != null)
            {
                var userJson = user.ToJson();
                var userJsonEncrypted = userJson.Encrypt();
                _session.SetString(SessionKeyUser, userJsonEncrypted);
            }
        }

        public void Remove(string key)
        {
            _session.Remove(key);
        }
        #endregion
    }
}
