<?php

namespace CoffeeHouse\Controllers;

use Illuminate\Routing\Controller;

class BaseController extends Controller
{
	private static $memcached = null;

	/**
	 * Setup the layout used by the controller.
	 *
	 * @return void
	 */
	protected function setupLayout()
	{
		if ( ! is_null($this->layout))
		{
			$this->layout = View::make($this->layout);
		}
	}

	/**
	 * @return Memcached
	 */
	protected function getMemcachedClient()
	{
		if (self::$memcached === null) {
			$client = new \Memcached();
			$client->addServer('memcached', '11211');
			self::$memcached = $client;
		}
		return self::$memcached;
	}
}
