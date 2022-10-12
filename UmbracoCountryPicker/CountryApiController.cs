using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Extensions;
using UmbracoKeyValuePropertyEditor;
using UmbracoMongoSmsDataServices;

namespace UmbracoCountryPicker
{

	[PluginController("UmbracoCountryPicker")]
	public class CountryApiController : KeyValueUmbracoPropertyEditorController
	{
		private readonly UmbracoHelper _umbracoHelper;

		public CountryApiController(UmbracoHelper umbracoHelper)
		{
			_umbracoHelper = umbracoHelper;
		}

		public override IOrderedEnumerable<KeyValuePair<string, string>> GetKeyValueList(int nodeId, string propertyAlias, int uniqueFilter = 0, int allowNull = 0)
		{
			try
			{
				string[] usedUpCountryCodes = Array.Empty<string>();
				try
				{
					var parent = _umbracoHelper.Content(nodeId).Parent;
					usedUpCountryCodes = (parent == null ? _umbracoHelper.Content(nodeId).Children.Where(c => c.Id != nodeId).Select(c => c.Value<string>(propertyAlias)?.ToLowerInvariant()) : parent.Children.Where(c => c.Id != nodeId).Select(c => c.Value<string>(propertyAlias)?.ToLowerInvariant()).Union(_umbracoHelper.Content(nodeId).Children.Where(c => c.Id != nodeId).Select(c => c.Value<string>(propertyAlias)?.ToLowerInvariant()))).ToArray();
				}
				catch { uniqueFilter = 0; }
				var countryIsoCodes = SmsDataService.GetAllCountryPhoneCodesDictionary.Keys.Select(k => k);
				if (uniqueFilter == 1)
				{
					countryIsoCodes = countryIsoCodes.Where(c => !usedUpCountryCodes.Contains(c));
				}
				if (allowNull == 1)
				{
					countryIsoCodes = countryIsoCodes.Prepend("");
				}
				return countryIsoCodes.ToDictionary(c => c, c => string.IsNullOrWhiteSpace(c) ? "NONE" : c).OrderBy(v => v.Key);
			}
			catch
			{
				return null;
			}
		}
	}
}
