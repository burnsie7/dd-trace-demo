<?php

namespace CoffeeHouse\Controllers;

use CoffeeHouse\Models\Coffee;
use CoffeeHouse\Service\PermissionChecker;
use DDTrace\GlobalTracer;
use Illuminate\Support\Facades\App;

class HomeController extends BaseController
{
    public function coffeeList()
    {
        $memcached = self::getMemcachedClient();
        $memcached->get('user:123456');

        /** @var PermissionChecker $permissionChecker */
        $permissionChecker = App::make(PermissionChecker::class);
        $permissionChecker->checkPermissions();

        Coffee::all();

        $this->getFavorites();

        return 'Coffees!';
    }

    public function getFavorites()
    {
        $tracer = GlobalTracer::get();
        $scope = $tracer->startActiveSpan('call_external_web_service');
        $span = $scope->getSpan();

        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL, 'http://php-coffeehouse-backend/favorites');
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
        curl_exec($ch);

        // Setting a fake exception to show an error on the trace
        $exception = new \Exception("HTTP/1.1 503 Service Temporarily Unavailable - Curl Errno returned 503");
        $span->setError($exception);

        $scope->close();
    }
}
