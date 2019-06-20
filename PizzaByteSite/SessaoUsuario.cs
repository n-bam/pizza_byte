using System;
using System.Web;

namespace PizzaByteSite
{
    public class SessaoUsuario
    {
        public SessaoUsuario()
        {
            this.Identificacao = "";
            this.NomeUsuario = "Não logado";
            this.IdUsuario = Guid.Empty;
            this.Administrador = false;
        }

        // Propriedades da sessão
        public string NomeUsuario { get; set; }
        public Guid IdUsuario { get; set; }
        public string Identificacao { get; set; }
        public bool Administrador { get; set; }

        /// <summary>
        /// Prepra a sessão
        /// </summary>
        public static SessaoUsuario SessaoLogin
        {
            get
            {
                SessaoUsuario sessao = (SessaoUsuario)HttpContext.Current.Session["_Sessao"];
                if (sessao == null)
                {
                    sessao = new SessaoUsuario();
                    HttpContext.Current.Session["_Sessao"] = sessao;
                }

                return sessao;
            }
        }
    }
}