import React from "react";
import Search from "./Search";
import Logo from "./Logo";

export default function Navbar() {
  return (
    <header className="sticky grid grid-cols-3 top-0 z-50   bg-white p-5 items-center text-gray-800 shadow-md">
      <Logo />
      <Search />
      <div className="float-right text-right">Login</div>
    </header>
  );
}
