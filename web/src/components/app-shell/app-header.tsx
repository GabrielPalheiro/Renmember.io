import Link from "next/link";

import { MainNav } from "@/components/app-shell/main-nav";

export function AppHeader() {
  return (
    <header className="border-b">
      <div className="mx-auto flex max-w-6xl items-center justify-between gap-4 px-4 py-3">
        <Link href="/" className="font-heading text-lg font-semibold">
          Renmember.io
        </Link>
        <MainNav />
      </div>
    </header>
  );
}
