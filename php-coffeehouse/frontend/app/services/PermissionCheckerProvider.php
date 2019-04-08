<?php

namespace CoffeeHouse\Service;

use DDTrace\GlobalTracer;
use Illuminate\Support\ServiceProvider;
use OpenTracing\Scope;

class PermissionCheckerProvider extends ServiceProvider
{
	public function register()
	{
		$this->app->instance(PermissionChecker::class, new PermissionChecker());
	}

	public function boot()
	{
		parent::boot();

		dd_trace(PermissionChecker::class, 'checkPermissions', function () {
			/** @var Scope $scope */
			$scope = GlobalTracer::get()->startActiveSpan('PermissionChecker@checkPermissions');
			$span = $scope->getSpan();
			$span->setTag('service.name', 'permission_checker');
			$span->setTag('resource', 'user:123456');
			$response = $this->checkPermissions();
			$scope->close();
			return $response;
		});
	}
}
