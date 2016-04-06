var app = angular.module('CarFinderApp').controller('CarFinderAppController', ['$scope', '$http', function ($scope, $http)
{
    $scope.years =[];
    $scope.make =[];
    $scope.models =[];
    $scope.trims =[];

    $scope.selectedYear = null;
    $scope.selectedMake = null;
    $scope.selectedModel = null;
    $scope.selectedTrim = null;
    $scope.carData = null;

    $scope.slideInterval = 4500;     //Controls the time lapse between image transition
    $scope.isCollapsed=true;         //Controls the collapsed state of the recalls panel


    $scope.getYears = function() {
        //declare options object (if necessary)
        // var options = {params:{ } }; //don't need one here, because there are no options to 
        //make request - this can be from a local service or from Angular $http service
        $http.get('api/years').then(function(response) {
            //assign result to a $scope variable
            $scope.years = response.data;
        });
    };


    $scope.getMakes = function(){
        //declare options object
        var options = {params:{year:$scope.selectedYear}}; 
        // pass the selected year to the API
        //make request - this can be from a local service or from Angular $http service
        $http.get('api/makes', options).then(function(response){
            //assign result to a $scope variable
            $scope.makes = response.data;
        });
    };
       
                    
    $scope.getModels = function(){
        var options = {params:{year:$scope.selectedYear, make:$scope.selectedMake}}; 
        $http.get('api/models', options).then(function(response){
            $scope.models = response.data;
        });
    };


    $scope.getTrims = function(){
        var options = {params:{year:$scope.selectedYear, make:$scope.selectedMake, model:$scope.selectedModel}};
        $http.get('api/trims', options).then (function(response){
            $scope.trims = response.data;
        });
    }; 
    

    $scope.getCars = function(){
        var options = {params:{year:$scope.selectedYear, make:$scope.selectedMake, model:$scope.selectedModel, trim:$scope.selectedTrim}};
        $http.get('api/cars', options).then(function (response){
            $scope.carData = response.data;
        });
    };


   $scope.getYears();
    
}]);