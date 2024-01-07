"use server";

import { fetchWrapper } from "@/lib/fetchWrapper";
import { Auction, PagedResult } from "@/types";
import { revalidatePath } from "next/cache";
import { FieldValues } from "react-hook-form";

export async function getAuctions(
  query: string
): Promise<PagedResult<Auction>> {
  const res = await fetchWrapper.get(`search${query}`);
  return res;
}

export async function createAuction(data: FieldValues) {
  return await fetchWrapper.post("auctions", data);
}

export async function updateAuction(data: FieldValues, id: string) {
  return await fetchWrapper.put(`auctions/${id}`, data);
}
export async function deleteAuction(id: string) {
  const res = await fetchWrapper.del(`auctions/${id}`);
  revalidatePath(`/Auctions/${id}`);
  return res;
}

export async function getDetailedView(id: string): Promise<Auction> {
  return await fetchWrapper.get(`auctions/${id}`);
}
