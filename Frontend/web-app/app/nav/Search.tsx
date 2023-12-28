"use client";

import { useParamsStore } from "@/hooks/useParamsStore";
import React, { useEffect, useState } from "react";
import { FaSearch } from "react-icons/fa";

export default function Search() {
  const setParams = useParamsStore((state) => state.setParams);
  const searchTerm = useParamsStore((state) => state.searchTerm);
  const [value, setValue] = useState(searchTerm);
  const onChange = (event: any) => {
    setValue(event.target.value);
  };
  useEffect(() => {
    setValue(searchTerm);
  }, [searchTerm]);

  const search = () => {
    setParams({ searchTerm: value });
  };
  return (
    <div className="w-[100%]">
      <div className="flex w-[100%] items-center border-2 rounded-full py-2 shadow-sm">
        <input
          onKeyDown={(e: any) => {
            if (e.key === "Enter") search();
          }}
          onChange={onChange}
          type="text"
          value={value}
          placeholder="Search for cars by make, model or color"
          className="flex-grow pl-5 bg-transparent focus:outline-none border-transparent focus:border-transparent focus:ring-0 text-sm text-gray-600"
        />
        <button onClick={search}>
          <FaSearch
            size={34}
            className="bg-red-400 text-white rounded-full p-2 cursor-pointer mx-2"
          />
        </button>
      </div>
    </div>
  );
}
