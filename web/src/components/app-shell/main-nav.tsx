"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

import { buttonVariants } from "@/components/ui/button";

const destinos = [
  { href: "/atividades", rotulo: "Lista" },
  { href: "/kanban", rotulo: "Kanban" },
  { href: "/calendario", rotulo: "Calendário" },
] as const;

export function MainNav() {
  const pathname = usePathname();

  return (
    <nav aria-label="Principal">
      <ul className="flex gap-1">
        {destinos.map(({ href, rotulo }) => {
          const estaAtivo = pathname.startsWith(href);

          return (
            <li key={href}>
              <Link
                href={href}
                aria-current={estaAtivo ? "page" : undefined}
                className={buttonVariants({
                  variant: estaAtivo ? "secondary" : "ghost",
                })}
              >
                {rotulo}
              </Link>
            </li>
          );
        })}
      </ul>
    </nav>
  );
}
