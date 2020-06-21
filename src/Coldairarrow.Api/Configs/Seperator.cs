using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Configs
{
    public class Seperator
    {
        /// <summary>
        /// 在 appsetting.json 中的位置
        /// </summary>
        public const string Section = "Assistant:Seperator";
        /// <summary>
        /// sku 关键词 分隔符，逗号分隔 ，默认 '+', '➕' 
        /// </summary>
        public string SkuKeySeperator { get; set; }

        /// <summary>
        /// sku 数量 分隔符，逗号分隔 ，默认 '+', '*' 
        /// </summary>
        public string SkuCountSeperator { get; set; }


        public string[] SkuKeySeperatorArray
        {
            get
            {
                return SkuKeySeperator?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.ToArray() ?? new string[] { "+" };
            }
        }
        public string[] SkuCountSeperatorArray
        {
            get
            {
                return SkuCountSeperator?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.ToArray() ?? new string[] { "+" };
            }
        }

    }
}
