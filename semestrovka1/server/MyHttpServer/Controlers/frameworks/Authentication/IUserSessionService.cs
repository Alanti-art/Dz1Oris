using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Controlers.frameworks.Authentication
{

    public interface IUserSessionService
    {
        /// <summary>
        /// Создаёт новый сеанс пользователя.
        /// </summary>
        /// <param name="userId">
        /// Идентификатор пользователя.
        /// </param>
        /// <returns>
        /// Уникальный идентификатор созданного сеанса.
        /// </returns>
        string CreateSession(int userId);

        /// <summary>
        /// Получает идентификатор пользователя
        /// по действующему сеансу.
        /// </summary>
        /// <param name="sessionId">
        /// Идентификатор сеанса.
        /// </param>
        /// <returns>
        /// Идентификатор пользователя или null,
        /// если сеанс отсутствует либо истёк.
        /// </returns>
        int? GetUserId(string? sessionId);

        /// <summary>
        /// Завершает пользовательский сеанс.
        /// </summary>
        /// <param name="sessionId">
        /// Идентификатор сеанса.
        /// </param>
        void RemoveSession(string? sessionId);
    }

}
