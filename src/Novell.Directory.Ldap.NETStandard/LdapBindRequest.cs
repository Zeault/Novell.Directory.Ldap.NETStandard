﻿/******************************************************************************
* The MIT License
* Copyright (c) 2003 Novell Inc.  www.novell.com
*
* Permission is hereby granted, free of charge, to any person obtaining  a copy
* of this software and associated documentation files (the Software), to deal
* in the Software without restriction, including  without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to  permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*******************************************************************************/

using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;
using System.Threading;

namespace Novell.Directory.Ldap
{
    /// <summary>
    ///     Represents a simple bind request.
    /// </summary>
    /// <seealso cref="LdapConnection.SendRequestAsync(LdapMessage,LdapMessageQueue,CancellationToken)"/>
    /// <seealso cref="LdapConnection.SendRequestAsync(LdapMessage,LdapMessageQueue,LdapConstraints,CancellationToken)"/>
    /*
     *       BindRequest ::= [APPLICATION 0] SEQUENCE {
     *               version                 INTEGER (1 .. 127),
     *               name                    LdapDN,
     *               authentication          AuthenticationChoice }
     */
    public class LdapBindRequest : LdapMessage
    {
        public override DebugId DebugId { get; } = DebugId.ForType<LdapBindRequest>();

        /// <summary>
        ///     Constructs a simple bind request.
        /// </summary>
        /// <param name="version">
        ///     The Ldap protocol version, use Ldap_V3.
        ///     Ldap_V2 is not supported.
        /// </param>
        /// <param name="dn">
        ///     If non-null and non-empty, specifies that the
        ///     connection and all operations through it should
        ///     be authenticated with dn as the distinguished
        ///     name.
        /// </param>
        /// <param name="passwd">
        ///     If non-null and non-empty, specifies that the
        ///     connection and all operations through it should
        ///     be authenticated with dn as the distinguished
        ///     name and passwd as password.
        /// </param>
        /// <param name="cont">
        ///     Any controls that apply to the simple bind request,
        ///     or null if none.
        /// </param>
        public LdapBindRequest(int version, string dn, byte[] passwd, LdapControl[] cont)
            : base(
                BindRequest,
                new RfcBindRequest(new Asn1Integer(version), new RfcLdapDn(dn),
                    new RfcAuthenticationChoice(new Asn1Tagged(
                        new Asn1Identifier(Asn1Identifier.Context, false, 0),
                        new Asn1OctetString(passwd), false))), cont)
        {
        }

        /// <summary>
        ///     Retrieves the Authentication DN for a bind request.
        /// </summary>
        /// <returns>
        ///     the Authentication DN for a bind request.
        /// </returns>
        public string AuthenticationDn => Asn1Object.RequestDn;

        /// <summary>
        ///     Return an Asn1 representation of this add request.
        ///     #return an Asn1 representation of this object.
        /// </summary>
        public override string ToString()
        {
            return Asn1Object.ToString();
        }
    }
}
