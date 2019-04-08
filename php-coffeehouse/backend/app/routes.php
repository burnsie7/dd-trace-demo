<?php

/*
|--------------------------------------------------------------------------
| Application Routes
|--------------------------------------------------------------------------
|
| Here is where you can register all of the routes for an application.
| It's a breeze. Simply tell Laravel the URIs it should respond to
| and give it the Closure to execute when that URI is requested.
|
*/

Route::get('/', ['uses' => 'CoffeeHouse\Controllers\HomeController@showWelcome', 'as' => 'home_page']);
Route::get('/favorites', ['uses' => 'CoffeeHouse\Controllers\HomeController@favorites', 'as' => 'favorites']);
