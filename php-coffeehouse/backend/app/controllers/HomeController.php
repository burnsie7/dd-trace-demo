<?php

namespace CoffeeHouse\Controllers;

use CoffeeHouse\Models\Favorite;

class HomeController extends BaseController {

	/*
	|--------------------------------------------------------------------------
	| Default Home Controller
	|--------------------------------------------------------------------------
	|
	| You may wish to use controllers instead of, or in addition to, Closure
	| based routes. That's great! Here is an example controller method to
	| get you started. To route to this controller, just add the route:
	|
	|	Route::get('/', 'HomeController@showWelcome');
	|
	*/

	public function showWelcome()
	{
		return View::make('hello');
	}

	public function favorites()
	{
		Favorite::all();
		$memcached = $this->getMemcachedClient();
		$memcached->add('favorite:user:123456', 'strong');

		usleep(400 * 1000);

		return 'Favorites!';
	}
}
