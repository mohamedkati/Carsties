"use client";

import React, { Fragment, useEffect, useState } from "react";
import AuctionCard from "./AuctionCard";
import { Auction, PagedResult } from "@/types";
import AppPagination from "../components/AppPagination";
import { getAuctions } from "../actions/auctionActions";
import Filters from "./Filters";
import { useParamsStore } from "@/hooks/useParamsStore";
import { useShallow } from "zustand/react/shallow";
import queryString from "query-string";
import { shallow } from "zustand/shallow";
import EmptyFilter from "../components/EmptyFilter";

export default function Listings() {
  // const [pageNumber, setPageNumber] = useState(1);
  const [auctions, setAuctions] = useState<PagedResult<Auction> | undefined>(
    undefined
  );
  // const [pageSize, setPageSize] = useState(4);

  const params = useParamsStore(
    (state) => ({
      pageNumber: state.pageNumber,
      pageSize: state.pageSize,
      searchTerm: state.searchTerm,
      orderBy: state.orderBy,
      filterBy: state.filterBy,
    }),
    shallow
  );
  const setParams = useParamsStore((state) => state.setParams);

  const url = queryString.stringifyUrl({ url: "", query: params });

  const setPageNumber = (pageNumber: number) => {
    setParams({ pageNumber });
  };

  useEffect(() => {
    getAuctions(url).then((res) => setAuctions(res));
  }, [url]);

  return (
    <Fragment>
      <Filters />
      {auctions?.totalCount == 0 && <EmptyFilter showReset />}
      <div className="grid grid-cols-4 gap-6">
        {auctions &&
          auctions.results.map((auction) => {
            return <AuctionCard auction={auction} key={auction.id} />;
          })}
      </div>
      <div className="flex justify-center">
        {auctions && auctions.totalCount !== 0 && (
          <AppPagination
            currentPage={params.pageNumber}
            pageCount={auctions.pageCount}
            changePageNumber={setPageNumber}
          />
        )}
      </div>
    </Fragment>
  );
}
