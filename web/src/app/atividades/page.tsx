import type { Metadata } from "next";

import { PageHeader } from "@/components/app-shell/page-header";

export const metadata: Metadata = { title: "Atividades" };

export default function AtividadesPage() {
  return (
    <PageHeader
      titulo="Atividades"
      descricao="A lista de atividades chega na Fatia 1."
    />
  );
}
