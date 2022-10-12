angular.module("umbraco").controller("UmbracoCountryPickerController", function ($scope, $routeParams, $http, localizationService) {
	if (!$scope.model) {
		$scope.model = {};
	} 

	$scope.loadInProgress = true;
	$scope.displayValue = $scope.model.value;
	$scope.inEditState = !!$routeParams.create;

	var localizeList = [
		"UmbracoCountryPicker_Edit"
	];
	
	$scope.translations = {};

	$scope.onEdit = function () {
		$scope.inEditState = true;
	}
	
	
	$scope.model.list = {};
	$http.get("/umbraco/backoffice/UmbracoCountryPicker/CountryApi/GetKeyValueList?nodeId=" + $routeParams.id + "&propertyAlias=" + $scope.model.alias + "&uniqueFilter=" + ($scope.model.config.uniqueFilter || 0) + "&allowNull=" + ($scope.model.config.allowNull || 0)).then(function (response) {
		$scope.model.list = response.data;

		for (var key in $scope.model.list) {
			if (!$scope.model.list.hasOwnProperty(key)) continue;
			localizeList.push("UmbracoCountryPicker_" + key);
		}
		localizationService.localizeMany(localizeList).then(function (data) {
			for (var i = 0; i < localizeList.length; ++i) {
				$scope.translations[localizeList[i]] = data[i];
			}
		});

		Object.keys($scope.model.list).forEach(key => {
			$scope.model.list[key] = $scope.translations["UmbracoCountryPicker_" + key];
		});
		var valueFromList = _.find($scope.model.list, function (item) { return item.Key === $scope.model.value });
		if (valueFromList) { $scope.displayValue = valueFromList.Value };
		$scope.loadInProgress = false;
	}, function (err) {
		$scope.loadInProgress = false;
		});

	$scope.$on("formSubmitting", function (ev, args) {
		if (args.action === "save" || args.action === "publish") {
			$scope.inEditState = false;
		}
	});
});
