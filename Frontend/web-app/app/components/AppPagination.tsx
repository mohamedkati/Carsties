"use client";

import { Pagination } from "flowbite-react";
import React, { useState } from "react";

type Props = {
  currentPage: number;
  pageCount: number;
  changePageNumber(pageNumber: number): void;
};
export default function AppPagination({
  currentPage,
  pageCount,
  changePageNumber,
}: Props) {
  return (
    <Pagination
      currentPage={currentPage}
      onPageChange={(e) => {
        changePageNumber(e);
      }}
      totalPages={pageCount}
      layout="pagination"
      showIcons={true}
      className="text-blue-500 mb-5"
    />
  );
}
