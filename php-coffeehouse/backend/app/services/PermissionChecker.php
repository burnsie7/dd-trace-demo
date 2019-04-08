<?php

namespace CoffeeHouse\Service;

use DDTrace\GlobalTracer;

class PermissionChecker
{
	public function checkPermissions() {
		$span = GlobalTracer::get()->getActiveSpan();
		$span->setTag('permissions.user.id', '123456');
		$span->setTag('permissions.user.permission.name', 'list_coffees');
		$span->setTag('permissions.user.permission.granted', 'true');
		return true;
	}
}
