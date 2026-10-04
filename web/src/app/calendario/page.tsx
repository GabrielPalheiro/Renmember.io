import type { Metadata } from "next";

import { PageHeader } from "@/components/app-shell/page-header";

export const metadata: Metadata = { title: "Calendário" };

export default function CalendarioPage() {
  return (
    <PageHeader
      titulo="Calendário"
      descricao="Os calendários mensal e semanal chegam na Fatia 3."
    />
  );
}
