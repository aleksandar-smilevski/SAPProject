/// <reference path="C:\Users\Aleksandar\Desktop\SAP\SAPProject\SAP\Scripts/angular.js" />
var app = angular.module('SapApp', ['ngAnimate'], function ($locationProvider) {
    $locationProvider.html5Mode({ enabled: true, requireBase: false});
});

app.controller('FormController', function ($scope, $http, $location) {
    $scope.activeTab = 1;
    $scope.isAdding = false;
    $scope.hasValues = false;
    $scope.activePanel = -1;
    $scope.newQuestion = {};
    $scope.form = {};
    $scope.form.questions = [];
    $scope.survey = {};

    $scope.addQuestion = function () {
        $scope.isAdding = !$scope.isAdding;
        setEmptyObject();
    };

    $scope.loadSurvey = function () {
        var id = $location.path().split('/')[3];
        if (id != undefined) {
            $http.get("/api/Surveys/" + id)
            .then(function (response) {
                $scope.survey.survey = response.data;
                $scope.form.questions = $scope.survey.survey.Questions;
                $scope.form.title = $scope.survey.survey.Name;
                console.log($scope.form.questions);
            });
        }
    };
    $scope.deleteQuestion = function (id) {
        var del = confirm("Are you sure you want to delete the question?");
        console.log(id);
        if (del) {
            if (id != undefined) {
                $http.delete("/api/Questions/" + $scope.survey.survey.Survey_type + "/" + id)
                    .then(function () {
                        $scope.loadSurvey();
                    });
            }
        }
        
    }

    $scope.confirmQuestion = function () {
        $scope.isAdding = false;
        console.log($scope.newQuestion);
        $http.post("/api/Questions/" + $scope.survey.survey.Survey_type, JSON.stringify($scope.newQuestion))
            .then(function (response) {
                $scope.loadSurvey();
        })
        setEmptyObject();
    };

    //$scope.addValue = function (value) {
    //    console.log(value);
    //    $scope.newQuestion.radios.push(value);
    //};

    $scope.cancelQuestion = function () {
        var answer = confirm("Are you sure you want to cancel?");
        if (answer) {
            $scope.isAdding = false;
            setEmptyObject();
        }
    };

    $scope.editQuestion = function (id) {
        console.log(id);
        $scope.isAdding = !$scope.isAdding;
        if(id != undefined){
            console.log($scope.form.questions);
            $http.get("/api/Questions/" + $scope.survey.survey.Survey_type + "/" + id)
                .then(function (response) {
                    $scope.newQuestion = response.data;
                });
            //$scope.newQuestion = $scope.form.questions.find()
        }
    }
    $scope.typeChange = function () {
        if ($scope.newQuestion.type == "text" || $scope.newQuestion.type == "textarea" || $scope.newQuestion.type == null) {
            $scope.newQuestion.radios = [];
            $scope.hasValues = false;
        }
        else {
            $scope.hasValues = true;
        }
    };

    $scope.showPanel = function (panel) {
        if (panel == $scope.activePanel) {
            $scope.activePanel = -1;
        } else {
            $scope.activePanel = panel;
        }
    };

    $scope.showValuesForm = function () {
        return false;
    }

    $scope.isShowing = function (panel) {
        return $scope.activePanel === panel;
    };

    $scope.setTab = function (tab) {
        $scope.activeTab = tab;
    };

    $scope.isSet = function (tab) {
        return $scope.activeTab === tab;
    };

    function setEmptyObject() {
        $scope.newQuestion = {
            title: "",
            type: "",
            required: false,
            description: "",
            radios: [],
            survey_id: $scope.survey.survey.Id
        };
    }
});
app.controller('CategoryController', function ($scope, $http, $location) {

    $scope.showModal = false;

    $scope.openModelCategory = function () {
        $scope.showModal =true;
    };
    $scope.okModelCategory = function () {
        $scope.showModal = false;
    };

    $scope.cancelModelCategory = function () {
        $scope.showModal = false;
    };

    
});