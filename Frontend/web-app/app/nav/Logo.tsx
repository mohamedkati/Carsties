"use client";

import { useParamsStore } from "@/hooks/useParamsStore";
import { usePathname, useRouter } from "next/navigation";
import React from "react";
import { AiOutlineCar } from "react-icons/ai";

export default function Logo() {
  const resetSearch = useParamsStore((state) => state.reset);

  const router = useRouter();
  const pathname = usePathname();

  const doReset = () => {
    if (pathname !== "/") router.push("/");
    resetSearch();
  };

  return (
    <div
      className="flex items-center gap-2 w-[25%] text-3xl font-semibold text-red-500 cursor-pointer"
      onClick={doReset}
    >
      <AiOutlineCar size={34} />
      <div>Carsties Auctions</div>
    </div>
  );
}
