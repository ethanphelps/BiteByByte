import { FocusArea } from "@app/_models/focus-area";

export class Category {
  id: string;
  name: string;
  description: string;
  color: string;
  focusAreas: FocusArea[];
}
