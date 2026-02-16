export type Member = {
  id: string;
  dateOfBirth: string;
  imageUrl?: string;
  displayName: string;
  created: string;
  lastActive: string;
  gender: string;
  description: string;
  city: string;
  country: string;
}

export type Photo = {
  id: number;
  url: string;
  publicId: any;
  memberId: string;
}

export type EditableMember = {
  displayName: string;
  description?: string;
  city: string;
  country: string;
}

export class MemberParams {
  gender?: string;
  minAge = 18;
  maxAge = 99;
  pageNumber = 1;
  pageSize = 5;
  orderBy = 'lastActive';
}

